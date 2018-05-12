/////////////////////////////////////////////////////////////////////////////////////////////////////
//
// Audiokinetic Wwise generated include file. Do not edit.
//
/////////////////////////////////////////////////////////////////////////////////////////////////////

#ifndef __WWISE_IDS_H__
#define __WWISE_IDS_H__

#include <AK/SoundEngine/Common/AkTypes.h>

namespace AK
{
    namespace EVENTS
    {
        static const AkUniqueID AMB_CLAPOTS = 2666046328U;
        static const AkUniqueID AMB_FALAISE = 3906951489U;
        static const AkUniqueID AMB_INIT = 1166681548U;
        static const AkUniqueID AMB_PORT = 2498798531U;
        static const AkUniqueID AMB_PORT_MER = 366006314U;
        static const AkUniqueID AMB_VILLAGE = 2699383670U;
        static const AkUniqueID EVENT_HEPHA_ATK = 82851639U;
        static const AkUniqueID EVENT_INIT = 15463988U;
        static const AkUniqueID PLAY_AMB_FALAISE_MOULIN_3D_LOOP = 944459842U;
        static const AkUniqueID PLAY_AMB_VILLAGE_CIGALES_FORT_2D_LOOP = 2980582648U;
        static const AkUniqueID PLAY_FOOTSTEPS_HEAVY = 145704183U;
        static const AkUniqueID PLAY_FOOTSTEPS_LIGHT = 2827287670U;
        static const AkUniqueID PLAY_M_THEME_VILLAGE_ACC = 2192112178U;
        static const AkUniqueID PLAY_M_THEME_VILLAGE_BASS = 434724282U;
        static const AkUniqueID PLAY_M_THEME_VILLAGE_GTR1 = 1860382329U;
        static const AkUniqueID PLAY_M_THEME_VILLAGE_GTR2 = 1860382330U;
        static const AkUniqueID PLAY_M_THEME_VILLAGE_PERCUS = 2003000063U;
        static const AkUniqueID PLAY_M_THEME_VILLAGE_SAZ = 2894506583U;
        static const AkUniqueID PLAY_PASSBY_BOAT = 1842608103U;
        static const AkUniqueID PLAY_TEST_3D = 2738087332U;
        static const AkUniqueID TEST_M_VILLAGE_DIEGETIQUE = 2918004713U;
        static const AkUniqueID TRIGGERZONE_BEGIN_FALAISE = 659911621U;
        static const AkUniqueID TRIGGERZONE_END_FALAISE = 132196245U;
        static const AkUniqueID TRIGGERZONE_FALAISE = 274211791U;
        static const AkUniqueID TRIGGERZONE_PORT = 2638844973U;
        static const AkUniqueID TRIGGERZONE_PORT_MER = 2513123340U;
        static const AkUniqueID TRIGGERZONE_VILLAGE = 1600917764U;
    } // namespace EVENTS

    namespace STATES
    {
        namespace CLOCHES_PLAYEDONCE
        {
            static const AkUniqueID GROUP = 405198451U;

            namespace STATE
            {
                static const AkUniqueID CLOCHES_PLAYEDONCE_FALSE = 431645235U;
                static const AkUniqueID CLOCHES_PLAYEDONCE_TRUE = 1549226682U;
            } // namespace STATE
        } // namespace CLOCHES_PLAYEDONCE

        namespace GAMEPLAY_PHASE
        {
            static const AkUniqueID GROUP = 2407702553U;

            namespace STATE
            {
                static const AkUniqueID ACTIF = 2247125562U;
                static const AkUniqueID PASSIF = 2831715831U;
            } // namespace STATE
        } // namespace GAMEPLAY_PHASE

        namespace MAP_ZONE
        {
            static const AkUniqueID GROUP = 838332226U;

            namespace STATE
            {
                static const AkUniqueID FALAISE = 3168137712U;
                static const AkUniqueID PORT = 1608679832U;
                static const AkUniqueID PORT_MER = 1866337637U;
                static const AkUniqueID VILLAGE = 3945572659U;
            } // namespace STATE
        } // namespace MAP_ZONE

        namespace MAP_ZONE_FALAISE
        {
            static const AkUniqueID GROUP = 1237485236U;

            namespace STATE
            {
                static const AkUniqueID FALAISE_BEGIN = 2124917922U;
                static const AkUniqueID FALAISE_END = 3997991830U;
            } // namespace STATE
        } // namespace MAP_ZONE_FALAISE

    } // namespace STATES

    namespace SWITCHES
    {
        namespace SW_FOOTSTEPS_TEXTURE
        {
            static const AkUniqueID GROUP = 3069673431U;

            namespace SWITCH
            {
                static const AkUniqueID DIRT = 2195636714U;
                static const AkUniqueID METAL = 2473969246U;
                static const AkUniqueID STONE = 1216965916U;
                static const AkUniqueID WOOD = 2058049674U;
            } // namespace SWITCH
        } // namespace SW_FOOTSTEPS_TEXTURE

        namespace SW_INTERACTIVE_OBJECTS
        {
            static const AkUniqueID GROUP = 3068589479U;

            namespace SWITCH
            {
                static const AkUniqueID DEBRIS = 459611840U;
                static const AkUniqueID DESTRUCTION = 2446376615U;
                static const AkUniqueID IMPACT = 3257506471U;
            } // namespace SWITCH
        } // namespace SW_INTERACTIVE_OBJECTS

        namespace SW_M_VBG_PROGRESS
        {
            static const AkUniqueID GROUP = 1698089957U;

            namespace SWITCH
            {
                static const AkUniqueID FINAL = 565529991U;
                static const AkUniqueID INTRO = 1125500713U;
                static const AkUniqueID THEME = 1319017392U;
                static const AkUniqueID THEMEACC = 1935897315U;
                static const AkUniqueID THEMPROG = 4037860975U;
            } // namespace SWITCH
        } // namespace SW_M_VBG_PROGRESS

    } // namespace SWITCHES

    namespace GAME_PARAMETERS
    {
        static const AkUniqueID DEBUG_SPEEDPLAYBACK = 2967529081U;
        static const AkUniqueID RTPC_DISTVILLAGE = 3879816141U;
        static const AkUniqueID RTPC_GAMEDEFINED_DISTANCE_MOULIN = 1846987439U;
    } // namespace GAME_PARAMETERS

    namespace BANKS
    {
        static const AkUniqueID INIT = 1355168291U;
        static const AkUniqueID VBG_MAIN_SBK = 3373090871U;
    } // namespace BANKS

    namespace BUSSES
    {
        static const AkUniqueID AMB = 1117531639U;
        static const AkUniqueID CHARA = 434268486U;
        static const AkUniqueID FALAISE = 3168137712U;
        static const AkUniqueID HEPHA = 2461340601U;
        static const AkUniqueID M = 84696434U;
        static const AkUniqueID MASTER_AUDIO_BUS = 3803692087U;
        static const AkUniqueID OBJECTS = 1695690031U;
        static const AkUniqueID PORT = 1608679832U;
        static const AkUniqueID RVB = 695384145U;
        static const AkUniqueID SFX = 393239870U;
        static const AkUniqueID SFX_DRY = 129293648U;
        static const AkUniqueID SFX_RVB = 194138449U;
        static const AkUniqueID VILLAGE = 3945572659U;
    } // namespace BUSSES

    namespace AUX_BUSSES
    {
        static const AkUniqueID AC_FALAISE = 2844349151U;
        static const AkUniqueID AC_PORT = 2055135965U;
        static const AkUniqueID AC_VILLAGE = 2725865684U;
    } // namespace AUX_BUSSES

    namespace AUDIO_DEVICES
    {
        static const AkUniqueID COMMUNICATION = 530303819U;
        static const AkUniqueID NO_OUTPUT = 2317455096U;
        static const AkUniqueID SYSTEM = 3859886410U;
    } // namespace AUDIO_DEVICES

}// namespace AK

#endif // __WWISE_IDS_H__
